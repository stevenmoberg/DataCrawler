﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCrawler
{
    public class InputQueue
    {
        public static ProcessQueue<Input> Instance { get; } = new ProcessQueue<Input>();
    }
}
