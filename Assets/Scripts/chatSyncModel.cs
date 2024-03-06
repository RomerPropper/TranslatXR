using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RealtimeModel]
public partial class chatSyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _chatText;

    [RealtimeProperty(2, true, false)]
    private string _lang;
}
