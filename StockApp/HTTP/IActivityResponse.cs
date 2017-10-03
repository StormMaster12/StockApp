using System.Collections.Generic;

namespace StockApp.HTTP
{
    interface IActivityResponse

    {
        void proccessFinish(List<tescoApiJson> jsonList);
    }
}