using MU.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using WebZen.Network;

namespace MU.Network.Auth
{
    public interface IAuthMessage
    { }

    public class AuthMessageFactory : MessageFactory<CSOpCode, IAuthMessage>
    {
        public AuthMessageFactory( )
        {
            Register<CIDAndPass>(CSOpCode.Login);
            Register<SLoginResult>(CSOpCode.Login);
            Register<CCharacterList>(CSOpCode.CharacterList);

            
            Register<SCharacterList>(CSOpCode.CharacterList);
          
            Register<CCharacterMapJoin>(CSOpCode.JoinMap);
            Register<CCharacterMapJoin2>(CSOpCode.JoinMap2);
            Register<SCharacterMapJoin>(CSOpCode.JoinMap);
            Register<SCharacterMapJoin2>(CSOpCode.JoinMap2);
            ChangeType<CIDAndPass>(CSOpCode.Login, typeof(CIDAndPass));

            Register<CServerList>(CSOpCode.ChannelList);
            Register<SServerList>(CSOpCode.ChannelList);
            Register<SEnableCreation>(CSOpCode.EnableCreate);
            Register<CCharacterCreate>(CSOpCode.CharacterCreate);
            Register<CCharacterDelete>(CSOpCode.CharacterDelete);
            Register<CServerMove>(CSOpCode.ServerMoveAuth);

            // S2C
            Register<SJoinResult>(CSOpCode.JoinResult);
             
            Register<SCharacterCreate>(CSOpCode.CharacterCreate);
            Register<SCharacterDelete>(CSOpCode.CharacterDelete);
           
            
            Register<SServerMove>(CSOpCode.ServerMove);
        }
    }
}
