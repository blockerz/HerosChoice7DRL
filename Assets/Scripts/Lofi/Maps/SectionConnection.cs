using System;
using System.Collections;
using System.Collections.Generic;

namespace Lofi.Maps
{
    public class SectionConnection
    {
        public Section From { get; set; }
        public Section To { get; set; }
        public Direction Direct { get; set; }
        
        public SectionConnection(Section from, Section to, Direction direction)
        {
            From = from;
            To = to;
            Direct = direction;
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                SectionConnection s = (SectionConnection)obj;
                return (From == s.From) && (To == s.To) && (Direct == s.Direct);
            }
        }
    }
}