using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StockApp.HTTP
{
    interface IActivityResponse

    {
        void proccessFinish(ObservableCollection<tescoApiJson> jsonList);
    }
}