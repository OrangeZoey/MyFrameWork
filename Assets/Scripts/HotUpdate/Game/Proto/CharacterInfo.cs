using System.Collections.Generic;

 
    public class CharacterInfo
    {
        public int configID;
        public CharacterTeam team;
        public List<SkillInfo> skills;
        public Dictionary<CharacterAttributeType, long> characterAttributes;
        public List<ItemInfo> equipments;
    }
 
