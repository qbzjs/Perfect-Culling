// Perfect Culling (C) 2021 Patrick König
//

using System.Collections.Generic;
using UnityEngine;

namespace Koenigz.PerfectCulling
{
    public class PerfectCullingSceneGroup : PerfectCullingMonoGroup
    {
        [SerializeField] private Renderer[] renderers = System.Array.Empty<Renderer>();
        [SerializeField] private UnityEngine.Behaviour[] behaviours = System.Array.Empty<UnityEngine.Behaviour>();
        
        public override List<Renderer> Renderers
        {
            get
            {
                List<Renderer> rs = new List<Renderer>((renderers != null) ? renderers : System.Array.Empty<Renderer>());

                rs.RemoveAll((r) => r == null);

                return rs;
            }
        }

        public override List<UnityEngine.Behaviour> UnityBehaviours
        {
            get
            {
                List<UnityEngine.Behaviour> rs = new List<UnityEngine.Behaviour>((behaviours != null) ? behaviours : System.Array.Empty<UnityEngine.Behaviour>());

                rs.RemoveAll((r) => r == null);

                return rs;
            }
        }

        public void SetRenderers(Renderer[] newRenderers)
        {
            renderers = newRenderers;
        }

        public override void PreSceneSave(PerfectCullingBakingBehaviour bakingBehaviour)
        {
        }

        public override void PreBake(PerfectCullingBakingBehaviour bakingBehaviour)
        {
        }

        public override void PostBake(PerfectCullingBakingBehaviour bakingBehaviour)
        {
        }
    }
}