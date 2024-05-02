using RequestBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request_BuyChipsView : BaseViewRequest
{
    BuyChipsView thisView;

    public override void Awake()
    {
        base.Awake();
    }

    public override void RegisterBroadcast()
    {
       
    }

    public override void SetThisView()
    {
        thisView = thisBaseView as BuyChipsView;
    }

    public override void SendRequest(MainPack pack)
    {
        base.SendRequest(pack);
    }

    public override void HandleRequest(MainPack pack)
    {
        
    }
}
