using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectOriginality.Party;

namespace ProjectOriginality
{
    public static class Global
    {
        public static List<PartyMember> PlayerParty { get; } = new List<PartyMember>();

        public static void Assert(bool test)
        {
            if (!test)
            {
                throw new Exception("Exception failed.");
            }
        }

        public static void Assert(bool test, string message)
        {
            if (!test)
            {
                throw new Exception(message);
            }
        }
    }
}
