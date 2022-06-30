// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.QnA;

namespace Microsoft.LibraryBotUtn.QnA
{
    public interface IQnAMakerServices
    {
        QnAMaker _qnamaker { get; set; }
    }
}
