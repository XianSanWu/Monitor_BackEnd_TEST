namespace Models.Dto.Responses
{
    /// <summary>
    /// 包含佇列訊息、權限及相關資訊的回應結構。
    /// </summary>
    public class MsmqQueueResponse
    {
        /// <summary>
        /// 描述佇列詳細資訊的回應結構，包含佇列基本資料、訊息和權限。
        /// </summary>
        public class MsmqQueueDetailsResponse
        {
            /// <summary> 佇列詳細資料的集合。  </summary>
            public List<QueueInfoResponse> Value { get; set; } = new();

            /// <summary> 回應中的資料筆數。  </summary>
            public int Count { get; set; }
        }

        /// <summary>
        /// 表示佇列的基本資訊，包括佇列名稱、訊息數量、佇列大小以及相關的日誌資訊。
        /// </summary>
        public class QueueInfoResponse
        {
            /// <summary> 佇列名稱，唯一標識該佇列。 </summary>
            public string QueueName { get; set; } = string.Empty;

            /// <summary> 佇列中的訊息數量，表示目前累積待處理的訊息數。 </summary>
            public int MessagesInQueue { get; set; }

            /// <summary> 佇列中的總字節數，代表佇列中所有訊息所佔的空間大小。 </summary>
            public int BytesInQueue { get; set; }

            /// <summary> 佇列日誌中的總字節數，通常用於備份或日誌目的。 </summary>
            public int BytesinJournalQueue { get; set; }

            /// <summary> 佇列日誌中的訊息數量，記錄已處理的或歷史訊息。 </summary>
            public int MessagesInJournalQueue { get; set; }

            /// <summary> 伺服器名稱或主機名稱，表示佇列所在的計算機。 </summary>
            public string PSComputerName { get; set; } = string.Empty;
        }

    }
}
