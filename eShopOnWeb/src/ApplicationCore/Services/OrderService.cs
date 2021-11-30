using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAsyncRepository<Order> _orderRepository;
        private readonly IUriComposer _uriComposer;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IAsyncRepository<CatalogItem> _itemRepository;
        private readonly IExternalApiService _externalApiService;
        private readonly IOrderBusService _orderBusService;

        private readonly string DeliveryOrderProcessorUrl = "https://delivery-order-processor.azurewebsites.net/api/DeliveryOrderProcessor";

        public OrderService(IAsyncRepository<Basket> basketRepository,
            IAsyncRepository<CatalogItem> itemRepository,
            IAsyncRepository<Order> orderRepository,
            IUriComposer uriComposer,
            IExternalApiService externalApiService,
            IOrderBusService orderBusService)
        {
            _orderRepository = orderRepository;
            _uriComposer = uriComposer;
            _basketRepository = basketRepository;
            _itemRepository = itemRepository;
            _externalApiService = externalApiService;
            _orderBusService = orderBusService;
        }

        public async Task CreateOrderAsync(int basketId, Address shippingAddress)
        {
            var basketSpec = new BasketWithItemsSpecification(basketId);
            var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);

            Guard.Against.NullBasket(basketId, basket);
            Guard.Against.EmptyBasketOnCheckout(basket.Items);

            var catalogItemsSpecification = new CatalogItemsSpecification(basket.Items.Select(item => item.CatalogItemId).ToArray());
            var catalogItems = await _itemRepository.ListAsync(catalogItemsSpecification);

            var items = basket.Items.Select(basketItem =>
            {
                var catalogItem = catalogItems.First(c => c.Id == basketItem.CatalogItemId);
                var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, _uriComposer.ComposePicUri(catalogItem.PictureUri));
                var orderItem = new OrderItem(itemOrdered, basketItem.UnitPrice, basketItem.Quantity);
                return orderItem;
            }).ToList();

            var order = new Order(basket.BuyerId, shippingAddress, items);
            await _orderRepository.AddAsync(order);

            var orderProjection = new Entities.OrderApi.Order
            {
                Id = order.Id.ToString(),
                Address = new Entities.OrderApi.OrderAddress
                {
                    City = order.ShipToAddress.City,
                    Country = order.ShipToAddress.Country,
                    Street = order.ShipToAddress.Street,
                    State = order.ShipToAddress.State,
                    ZipCode = order.ShipToAddress.ZipCode
                },
                Items = order.OrderItems.Select(x => new Entities.OrderApi.OrderItem
                {
                    Name = x.ItemOrdered.ProductName,
                    Price = x.UnitPrice.ToString(),
                    Count = x.Units
                }).ToArray(),
                FinalPrice = order.OrderItems.Sum(x => x.UnitPrice).ToString()
            };

            await _orderBusService.SendMessageInQueueAsync(orderProjection);
            await _externalApiService.RunRequest<Entities.OrderApi.Order>(DeliveryOrderProcessorUrl, orderProjection, null);
        }
    }
}
