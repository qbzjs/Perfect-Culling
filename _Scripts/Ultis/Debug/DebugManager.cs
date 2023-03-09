using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : Singleton<DebugManager>
{
   public void Show(string content)
   {
#if TEST_MODE
      PanelRoot.Show<DebugPopup>().SetContent(content);
#endif
   }
}
