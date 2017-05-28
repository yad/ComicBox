﻿using System.Collections.Generic;
using System.Linq;

namespace ComicBoxApi.App
{
    public class BookContainer<T>
    {
        public string Thumbnail { get; private set; }

        public IReadOnlyCollection<T> Collection { get; private set; }

        public BookContainer(string thumbnail, IEnumerable<T> collection)
        {
            Thumbnail = thumbnail;
            Collection = collection.ToArray();
        }
    }
}
