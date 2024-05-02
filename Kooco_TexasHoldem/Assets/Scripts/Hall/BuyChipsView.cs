using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RequestBuf;

public class BuyChipsView : BaseView
{
    Request_BuyChipsView thisRequestView;
    public override void SetRequestView()
    {
        thisRequestView = requestView as Request_BuyChipsView;
    }
}
