using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerInfoComponent : ECSComponent
{
    public UserInfo userInfo;

}

public class PlayerInfoComponentAwakeSystem : AwakeSystem<PlayerInfoComponent>
{
    public override void Awake(PlayerInfoComponent c)
    {
        c.userInfo = new UserInfo();

    }
}




