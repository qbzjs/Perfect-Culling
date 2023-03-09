// Perfect Culling (C) 2021 Patrick König
//

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Koenigz.PerfectCulling
{
    public class PerfectCullingBakerUnityHandle : PerfectCullingBakerHandle
    {
        public ComputeBuffer appendBuf;
        public ComputeBuffer countBuf;

        public int[] m_hash;

#if !UNITY_2021_1_OR_NEWER
        // We can re-use them because we can only call GetData, etc. on the main thread anyway.
        private static readonly int[] m_out = new int[PerfectCullingConstants.MaxRenderers];
#endif
        
        private static readonly int[] m_counterOutput = new int[1] { 0 };

        protected override void DoComplete()
        {
            ComputeBuffer.CopyCount(appendBuf, countBuf, 0);
                
            countBuf.GetData(m_counterOutput);

            int appendBufCount = m_counterOutput[0];
            
            indices = new ushort[appendBufCount];

            if (appendBufCount > 0)
            {
#if UNITY_2021_1_OR_NEWER
                // GetData seems to be unreliable in Unity 2021 (at least on Mac M1 Silicon). This seems to work much better.
                AsyncGPUReadbackRequest request = AsyncGPUReadback.Request(appendBuf, sizeof(int) * appendBufCount, 0, null);

                // This will most likely already complete immediately because the GPU should be done when we get here.
                request.WaitForCompletion();

                NativeArray<int> requestResult = request.GetData<int>();

                for (int i = 0; i < appendBufCount; ++i)
                {
                    int q = requestResult[i];

                    int b = q / (256 * 256);
                    q -= (b * 256 * 256);
                    int g = q / 256;
                    int r = q % 256;

                    // The value returned might actually overflow so we cannot use q directly
                    int index = (b * 256 * 256) + (g * 256) + r; //r + 256 * (g + 256 * b);

                    indices[i] = (ushort) m_hash[index];
                }

                requestResult.Dispose();
#else
                // Partial read
                appendBuf.GetData(m_out, 0, 0, appendBufCount);

                for (int i = 0; i < appendBufCount; ++i)
                {
                    int q = m_out[i];

                    int b = q / (256 * 256);
                    q -= (b * 256 * 256);
                    int g = q / 256;
                    int r = q % 256;

                    // The value returned might actually overflow so we cannot use q directly
                    int index = (b * 256 * 256) + (g * 256) + r; //r + 256 * (g + 256 * b);

                    indices[i] = (ushort) m_hash[index];
                }
#endif
                
                System.Array.Sort(indices);
            }

            appendBuf.Dispose();
            countBuf.Dispose();

            appendBuf = null;
            countBuf = null;
        }
    }
}