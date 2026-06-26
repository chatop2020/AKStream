using System;
using System.Collections.Generic;

namespace LibCommon.Structs.WebResponse.AKStreamKeeper
{
    [Serializable]
    public class ResKeeperCutMergeTaskStatusResponseList
    {
        List<ResKeeperCutMergeTaskStatusResponse> _cutMergeTaskStatusResponseList =
            new List<ResKeeperCutMergeTaskStatusResponse>();

        public List<ResKeeperCutMergeTaskStatusResponse> CutMergeTaskStatusResponseList
        {
            get => _cutMergeTaskStatusResponseList;
            set => _cutMergeTaskStatusResponseList = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}