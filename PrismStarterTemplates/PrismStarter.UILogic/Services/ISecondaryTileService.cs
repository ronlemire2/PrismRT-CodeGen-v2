// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace PrismStarter.UILogic.Services
{
    public interface ISecondaryTileService
    {
        bool SecondaryTileExists(string tileId);

        Task<bool> PinSquareSecondaryTile(string tileId, string shortName, string displayName, string arguments);

        Task<bool> PinWideSecondaryTile(string tileId, string shortName, string displayName, string arguments);

        Task<bool> UnpinTile(string tileId);

        void ActivateTileNotifications(string tileId, Uri tileContentUri, PeriodicUpdateRecurrence recurrence);
    }
}