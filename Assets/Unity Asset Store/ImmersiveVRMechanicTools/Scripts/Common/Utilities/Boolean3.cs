namespace ReliableSolutions.Unity.Common.Utilities
{
    [System.Serializable]
    public struct Boolean3
    {

        public bool x;
        public bool y;
        public bool z;

        public Boolean3(bool all)
        {
            this.x = all;
            this.y = all;
            this.z = all;
        }

        public Boolean3(bool x, bool y, bool z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool AnyTrue()
        {
            return x || y || z;
        }

        public bool this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new System.IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new System.IndexOutOfRangeException();
                }
            }
        }

        public override string ToString()
        {
            return "Boolean3(" + x + ", " + y + ", " + z + ")";
        }
    }
}
