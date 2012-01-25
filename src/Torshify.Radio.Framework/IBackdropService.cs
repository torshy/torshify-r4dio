using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torshify.Radio.Framework
{
    public interface IBackdropService
    {
        Task<IEnumerable<string>> Query(string artistName);

        bool TryGet(string artistName, out string fileName);

        bool TryGet(string artistName, out string[] fileNames);
    }
}