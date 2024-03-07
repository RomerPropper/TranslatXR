using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RealtimeModel]
public partial class chatSyncModel
{
    [RealtimeProperty(1, true, true)]
    private string _chatText;
}
