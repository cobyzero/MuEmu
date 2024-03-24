using MU.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using WebZen.Network;

namespace MU.Network.CashShop
{
    public interface ICashMessage
    { }

    public class CashShopMessageFactory : MessageFactory<CashOpCode, ICashMessage>
    {
        public CashShopMessageFactory()
        {
            //C2S
            Register<CCashOpen>(CashOpCode.CashOpen);
             Register<CCashInventoryItem>(CashOpCode.CashInventoryCount);
            Register<CCashItemBuy>(CashOpCode.CashItemBuy);

            // S2C
            
            Register<SCashPoints>(CashOpCode.CashPoints);
              
            Register<SCashInit>(CashOpCode.CashInit);
            Register<SCashVersion>(CashOpCode.CashVersion);
            Register<SCashBanner>(CashOpCode.CashBanner);
            Register<SCashOpen>(CashOpCode.CashOpen);
            Register<SCashInventoryItem>(CashOpCode.CashInventoryCount);
            Register<SCashItemBuy>(CashOpCode.CashItemBuy);
            Register<SCashItemList>(CashOpCode.CashItemList);
        }
    }
}
