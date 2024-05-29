using System.Collections.Generic;


public class SCCampCapability : BaseProtocol
{
    public int[] capability_list;
    public override void Init()
    {
        base.Init();
        this.msg_type = 7008;
    }
    public override void Decode()
    {
        capability_list = new int[4];
        for (int i = 0; i <= 3; i++)
        {
            this.capability_list[i] = MsgAdapter.ReadInt();
        }
        //    self.capability_list = { }
        //    for i = 0, 3 do
        //            self.capability_list[i] = MsgAdapter.ReadInt()

        //    end

        UnityLog.Info($"  SCCampCapability 解析完毕 {this.capability_list[0]},   " +
            $", {this.capability_list[1]},    " +
            $", {this.capability_list[2]},    " +
            $", {this.capability_list[3]}");

    }
}

