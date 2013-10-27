// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


namespace PrismStarter.UILogic
{
    public static class Constants
    {
        //public const string ServerAddress = "http://entitywebapi.azurewebsites.net";

        // IIS DELL15z
        //public const string ServerAddress = "http://192.168.1.1:8081";

        // IIS Express
        //public const string ServerAddress = "http://localhost:59444";

        // Visual Studio Development Server
        public const string ServerAddress = "http://localhost:56789";

        // We allow all Unicode letter characters as well as internal spaces and hypens, as long as these do not occur in sequences.
        public const string NAMES_REGEX_PATTERN = @"\A\p{L}+([\p{Zs}\-][\p{L}]+)*\z";

        // We allow all Unicode letter and numeric characters as well as internal spaces, as long as these do not occur in sequences.
        public const string ADDRESS_REGEX_PATTERN = @"\A[\p{L}\p{N}]+([\p{Zs}][\p{L}\p{N}]+)*\z";

        // We allow all Unicode umeric characters and hypens, as long as these do not occur in sequences.
        public const string NUMBERS_REGEX_PATTERN = @"\A\p{N}+([\p{N}\-][\p{N}]+)*\z";
        public const string SSN_REGEX_PATTERN = @"^\d{3}-?\d{2}-?\d{4}$";

        // bool
        public const string BOOLEAN_REGEX_PATTERN = @"True|true|False|false";

        // datetime
        public const string DATE_WITH_SLASHES_REGEX_PATTERN = @"^\d{1,2}\/\d{1,2}\/\d{4}$";
        public const string DATETIME_REGEX_PATTERN = @"^((((([13578])|(1[0-2]))[\-\/\s]?(([1-9])|([1-2][0-9])|(3[01])))|((([469])|(11))[\-\/\s]?(([1-9])|([1-2][0-9])|(30)))|(2[\-\/\s]?(([1-9])|([1-2][0-9]))))[\-\/\s]?\d{4})(\s((([1-9])|(1[02]))\:([0-5][0-9])((\s)|(\:([0-5][0-9])\s))([AM|PM|am|pm]{2,2})))?$";

        // int
        public const string POSITIVE_INTEGER_REGEX_PATTERN = @"^\d+$";
        public const string SIGNED_INTEGER_REGEX_PATTERN = @"^(\+|-)?\d+$";
        public const string SMALL_ZIP_REGEX_PATTERN = @"^\d{5}$";

        // decimal
        public const string MONEY_REGEX_PATTERN = @"^\-?\(?\$?\s*\-?\s*\(?(((\d{1,3}((\,\d{3})*|\d*))?(\.\d{1,4})?)|((\d{1,3}((\,\d{3})*|\d*))(\.\d{0,4})?))\)?$";

    }
}
