using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board
{
    [Serializable]
    public struct BlockDataStruct
    {
        public BlockView blockPrefab;
        public int percentage;
    }
}
