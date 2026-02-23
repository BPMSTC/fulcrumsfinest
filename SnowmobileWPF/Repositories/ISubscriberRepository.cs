using SnowmobileLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SnowmobileWPF.Repositories
{
    public interface ISubscriberRepository
    {
        List<Subscriber>? Search(SearchParams searchParams);
        List<Subscriber> Retrieve(int max);
        void Create(Subscriber subscriber, bool forceCreation = false);
        void Delete(Subscriber subscriber);
        void Update(Subscriber subscriber);
    }
}
