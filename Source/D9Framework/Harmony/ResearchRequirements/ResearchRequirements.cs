using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace D9Framework
{
    class ResearchRequirements : DefModExtension
    {
#pragma warning disable CS0649
        public List<ResearchReqProps> requirements;
        public int threshold;
#pragma warning restore CS0649
        List<ResearchReq> reqs;
        public bool Fulfilled => reqs.Where(x => x.Fulfilled).Count() >= threshold;
        public ResearchRequirements()
        {
            reqs = new List<ResearchReq>();
            foreach(ResearchReqProps rrp in requirements)
            {
                // initialize req with props
            }
        }
    }
    public abstract class ResearchReqProps
    {
        public ResearchProjectDef parent;
        public int boostAmount = 0;
        public Type reqClass;
    }
    public abstract class ResearchReq
    {
        public ResearchReqProps props;
        private bool fulfilled = false;
        public virtual bool Fulfilled => fulfilled;

        public void OnFulfilled()
        {
            if(props.boostAmount > 0) props.parent.AddProgress(props.boostAmount);
        }
    }
}
