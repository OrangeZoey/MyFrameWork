//using Nirvana;
//using System.Collections.Generic;
//using UnityEngine;


////--login server 的链接信息
////	self:ResetLoginServer()

////    self.login_server_host_name = "127.0.0.1"

////    self.login_server_host_port = 10300

////    -- game server 的链接信息
////	self:ResetGameServer()

////    self.game_server_host_name = "127.0.0.1"

////    self.game_server_host_port = 4777

////    -- cross server 的链接信息
////	self:ResetCrossServer()

////    self.cross_server_host_name = "127.0.0.1"

////    self.cross_server_host_port = 0
//public class GameTest : MonoBehaviour
//{
//    // Start is called before the first frame update
//    private NetClient netClient;
//    private void Awake()
//    {
//        netClient = new NetClient();
//    }
//    void Start()
//    {
//        ///链接登录服务器
//        netClient.Connect("127.0.0.1", 10300, (is_succ) =>
//        {
//            if (is_succ)
//            {
//                Debug.Log("Async Connect to game server Ret: status " + is_succ);
//                this.netClient.StartReceive();

//                this.netClient.ReceiveEvent += NetClient_ReceiveEvent;

//                SendTestMsgToLoginServer();
//            }

//        });
//    }


//    //protocol.rand_1 =     5318588    
//    //protocol.login_time =     1705457649    
//    //protocol.key =         
//    //protocol.rand_2 =     4808874    
//    //protocol.plat_fcm =     0    
//    //protocol.plat_name =     dev_aa
//    //protocol.plat_server_id =    1


//    //    MsgAdapter.WriteInt(self.rand_1)
//    //    MsgAdapter.WriteUInt(self.login_time)
//    //    MsgAdapter.WriteStrN(self.key, 32)
//    //	  MsgAdapter.WriteStrN(self.plat_name, 64)
//    //	  MsgAdapter.WriteInt(self.rand_2)
//    //    MsgAdapter.WriteShort(0)
//    //    MsgAdapter.WriteShort(self.plat_server_id)
//    private void SendTestMsgToLoginServer()
//    {
//        CSLoginReq msg = new CSLoginReq();
//        msg.rand_1 = 5318588;
//        msg.login_time = 1705457649;
//        msg.key = "";
//        msg.plat_name = "dev_aa";
//        msg.rand_2 = 4808874;
//        msg.plat_fcm = 0;
//        msg.plat_server_id = 1;
//        msg.EncodeAndSend();

//        //ByteBuffer byteBuf = new ByteBuffer();

//        /////WriteBegin    msgType ,0 
//        //byteBuf.WriteUShort(7056);
//        //byteBuf.WriteShort(0);

//        //// 1. WriteInt  self.rand_1   5318588
//        //byteBuf.WriteInt(5318588);

//        ////2. WriteUInt   self.login_time  1705457649
//        //byteBuf.WriteUInt(1705457649);

//        /////3. WriteStrN  WriteStrN(self.key, 32)    ""(空字符串)
//        //byteBuf.WriteString("", 32);

//        ////4. WriteStrN(self.plat_name, 64)   
//        //byteBuf.WriteString("dev_aa", 64);

//        /////5.WriteInt(self.rand_2)    4808874
//        //byteBuf.WriteInt(4808874);

//        /////6. WriteShort(0)  应该是plat_fcm 
//        //byteBuf.WriteShort(0);

//        /////7.WriteShort(self.plat_server_id)    1
//        //byteBuf.WriteShort(1);


//        //netClient.SendMsg(byteBuf.ToBytes());
//    }

//    private void NetClient_ReceiveEvent(byte[] message, uint length)
//    {

//        ByteBuffer byteBuf = new ByteBuffer(message);
//        ///读取消息类型（id），需要根据消息类型到协议池子中找到协议
//        ushort msgType = byteBuf.ReadUShort();
//        byteBuf.ReadUShort();
//        UnityEngine.Debug.Log("收到数据长度 = " + message.Length + "  lenght = " + length + "mstType = " + msgType);
//        switch (msgType)
//        {
//            case 7006:
//                parse7006(msgType, message, length, byteBuf);
//                break;

//            case 7008:
//                parse7008(msgType, message, length, byteBuf);//--各国平均战斗力
//                break;

//            case 7001:
//                parse7001(msgType, message, length, byteBuf);
//                break;
//            default:
//                break;
//        }
//        byteBuf.Close();

//    }

//    public class RoleList
//    {
//        public short result;
//        public int count;
//        public List<Role> role_list;
//    }
   

//    /// <summary>
//    /// -- 角色列表返回
//    /// </summary>
//    /// <param name="msgType"></param>
//    /// <param name="message"></param>
//    /// <param name="length"></param>
//    /// <param name="byteBuf"></param>
//    private void parse7001(ushort msgType, byte[] message, uint length, ByteBuffer byteBuf)
//    {
//        byteBuf.ReadShort();
//        RoleList roleList = new RoleList();
//        roleList.result = byteBuf.ReadShort();
//        roleList.count = byteBuf.ReadInt();
//        Debug.Log($" roleList.result = {roleList.result}  roleList.count ={roleList.count}");

//        roleList.role_list = new List<Role>();
//        for (int i = 0; i < roleList.count; i++)
//        {
//            Role role = new Role();
//            role.role_id = byteBuf.ReadInt();
//            role.role_name = byteBuf.ReadString(32);
//            role.avatar = byteBuf.ReadSByte();  ////readChar()---->readSByte()
//            role.sex = byteBuf.ReadSByte();
//            role.prof = byteBuf.ReadSByte();
//            role.country = byteBuf.ReadSByte();
//            role.camp = role.country;
//            role.level = byteBuf.ReadInt();
//            role.create_time = byteBuf.ReadUInt();
//            role.last_login_time = byteBuf.ReadUInt();
//            role.wuqi_id = byteBuf.ReadUShort();
//            role.shizhuang_wuqi = byteBuf.ReadSByte();
//            role.shizhuang_body = byteBuf.ReadSByte();
//            role.wing_used_imageid = byteBuf.ReadShort();
//            role.halo_used_imageid = byteBuf.ReadShort();
//            role.wuqi_use_type = byteBuf.ReadShort();
//            role.body_use_type = byteBuf.ReadShort();
//            role.shenbing_img_id = byteBuf.ReadShort();
//            role.shenbing_texiao_id = byteBuf.ReadShort();
//            role.baojia_img_id = byteBuf.ReadShort();
//            role.baojia_texiao_id = byteBuf.ReadShort();
//            role.fazhen_used_imageid = byteBuf.ReadShort();
//            byteBuf.ReadShort();
//            role.appearance = new RoleAppearance();
//          //  role.appearance.Decode(byteBuf);

//            roleList.role_list.Add(role);
//            Debug.Log($"role.role_id ={role.role_id} role.role_name = {role.role_name.Trim()} role.avatar = {role.avatar}  role.level = {role.level}");
//        }
//        //10:32:12.737 - 196: [protocolcommon/ userprotocol / protocol_070.lua:60]:self.role_list[i].role_id    1048977    self.role_list[i].role_name    彭峻峰 self.role_list[i].avatar    1    self.role_list[i].sex    1    self.role_list[i].prof    1    self.role_list[i].country    3    self.role_list[i].shizhuang_wuqi    0
//        //10:32:12.739-196: [protocolcommon/userprotocol/protocol_070.lua:60]:self.role_list[i].role_id    1049077    self.role_list[i].role_name    闾丘亦真    self.role_list[i].avatar    1    self.role_list[i].sex    0    self.role_list[i].prof    4    self.role_list[i].country    2    self.role_list[i].shizhuang_wuqi    0

//        //MsgAdapter.ReadShort()
//        //self.result = MsgAdapter.ReadShort()
//        //self.count = MsgAdapter.ReadInt()
//        //self.role_list = {}
//        //for i=1, self.count do
//        //	self.role_list[i] = {}
//        //	self.role_list[i].role_id = MsgAdapter.ReadInt()
//        //	self.role_list[i].role_name = MsgAdapter.ReadStrN(32)
//        //	self.role_list[i].avatar = MsgAdapter.ReadChar()
//        //	self.role_list[i].sex = MsgAdapter.ReadChar()
//        //	self.role_list[i].prof = MsgAdapter.ReadChar()
//        //	self.role_list[i].country = MsgAdapter.ReadChar()
//        //	self.role_list[i].camp = self.role_list[i].country
//        //	self.role_list[i].level = MsgAdapter.ReadInt()
//        //	self.role_list[i].create_time = MsgAdapter.ReadUInt()
//        //	self.role_list[i].last_login_time = MsgAdapter.ReadUInt()
//        //	self.role_list[i].wuqi_id = MsgAdapter.ReadUShort()
//        //	self.role_list[i].shizhuang_wuqi = MsgAdapter.ReadChar()
//        //	self.role_list[i].shizhuang_body = MsgAdapter.ReadChar()
//        //	self.role_list[i].wing_used_imageid = MsgAdapter.ReadShort()
//        //	self.role_list[i].halo_used_imageid = MsgAdapter.ReadShort()
//        //	self.role_list[i].wuqi_use_type = MsgAdapter.ReadShort()
//        //	self.role_list[i].body_use_type = MsgAdapter.ReadShort()
//        //	self.role_list[i].shenbing_img_id = MsgAdapter.ReadShort()
//        //	self.role_list[i].shenbing_texiao_id = MsgAdapter.ReadShort()
//        //	self.role_list[i].baojia_img_id = MsgAdapter.ReadShort()
//        //	self.role_list[i].baojia_texiao_id = MsgAdapter.ReadShort()
//        //	self.role_list[i].fazhen_used_imageid = MsgAdapter.ReadShort()
//        //	MsgAdapter.ReadShort()
//        //	self.role_list[i].appearance = ProtocolStruct.ReadRoleAppearance()
//        //end
//    }


//    /// <summary>
//    /// --各国平均战斗力
//    /// </summary>
//    /// <param name="msgType"></param>
//    /// <param name="message"></param>
//    /// <param name="length"></param>
//    /// <param name="byteBuf"></param>
//    private void parse7008(ushort msgType, byte[] message, uint length, ByteBuffer byteBuf)
//    {
//        //self.capability_list = { }
//        //for i = 0, 3 do
//        //        self.capability_list[i] = MsgAdapter.ReadInt()
//        //end

//        List<int> capability_list = new List<int>();
//        for (int i = 0; i <= 3; i++)
//        {
//            capability_list.Add(byteBuf.ReadInt());
//        }

//        Debug.Log($"capability_list[0] = {capability_list[0]} ，" +
//            $"capability_list[1] = {capability_list[1]}，" +
//            $"capability_list[2] = {capability_list[2]}，" +
//            $"capability_list[3] = {capability_list[3]} ");
//        ////09:49:37.584-198: [protocolcommon/userprotocol/protocol_070.lua:130]:0           13736857           53614078           10058916
//        ///         capability_list[0] = 0 ，capability_list[1] = 13736857，capability_list[2] = 53614078，capability_list[3] = 10058916 
//    }

//    private void parse7006(ushort msgType, byte[] message, uint length, ByteBuffer byteBuf)
//    {
//        int prof1_num = byteBuf.ReadInt();
//        int prof2_num = byteBuf.ReadInt();
//        int prof3_num = byteBuf.ReadInt();
//        int prof4_num = byteBuf.ReadInt();
//        Debug.Log($"prof1_num = {prof1_num}    prof2_num  = {prof2_num},   prof3_num = {prof3_num},  prof4_num = {prof4_num}");
//        //self.prof1_num = MsgAdapter.ReadInt()
//        //self.prof2_num = MsgAdapter.ReadInt()
//        //self.prof3_num = MsgAdapter.ReadInt()
//        //self.prof4_num = MsgAdapter.ReadInt()
//    }
//}
