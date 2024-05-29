 
    public struct BuffData
    {
        public int configID;
        public long casterEntityID;
        public long targetEntityID;
        public BuffOverrideData? overrideData;
    }

    public struct BuffOverrideData
    {
        public float? duration;
    }
 