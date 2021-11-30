﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OrderItemsReserver.Models
{
    public class OrderAddress
    {
        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}
