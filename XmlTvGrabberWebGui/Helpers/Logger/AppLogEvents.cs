namespace XmlTvGrabberWebGui.Helpers.Logger
{
    public class AppLogEvents
    {
        #region Informations

        public const int GrabberAutoStart = 1000;
        public const int GrabberCreateTempFolder = 1005;
        public const int GrabberCreateTempFolderCleanUp = 1010;
        public const int GrabberFileDownload = 1015;
        public const int GrabberExtractZip = 1020;
        public const int GrabberExtractGZip = 1025;
        public const int GrabberProcessingFile = 1030;
        public const int GrabberFilteringCategories = 1035;
        public const int GrabberInsertMissingCategories = 1036;
        public const int GrabberFilteringResult = 1040;
        public const int GrabberCreateOutputFile = 1045;
        public const int GrabberUnixSocketConnection = 1050;
        public const int GrabberUnixSocketSend = 1055;
        public const int GrabberEnd = 1060;
        public const int GrabberNextXmlUrl = 1065;

        public const int XmlCatgoryCreated = 2000;
        public const int XmlCatgoryUpdated = 2005;
        public const int XmlCatgoryDeleted = 2010;

        public const int TvHeadendCatgoryCreated = 3000;
        public const int TvHeadendCatgoryUpdated = 3005;
        public const int TvHeadendCatgoryDeleted = 3010;

        public const int TvHeadendServiceStop = 3500;
        public const int TvHeadendEpgDelete = 3505;
        public const int TvHeadendServiceStart = 3510;

        public const int ConfigurationUpdated = 4000;

        #endregion

        #region Warnings

        public const int GrabberMissingConfiguration = 9000;
        public const int GrabberNoValidFileFound = 9001;
        public const int GrabberUnsupportedFileExtension = 9005;
        public const int GrabberNoXmlFile = 9010;
        public const int GrabberXmlFilesEmpty = 9011;
        public const int GrabberUnsupportedXmlFile = 9015;
        public const int GrabberEmptyPrograms = 9020;
        public const int GrabberEmptyXmlOutputFile = 9025;

        public const int ConfigurationEpgPath = 9200;
        public const int ConfigurationEpgNotFound = 9205;

        #endregion

        #region Errors

        public const int GrabberException = 10000;
        public const int GrabberFileDownloadException = 10005;

        public const int XmlCatgorySaveError = 10100;
        public const int XmlCatgoryMissingId = 10105;

        public const int TvHeadendCatgorySaveError = 10200;
        public const int TvHeadendCatgoryMissingId = 10205;

        public const int ConfigurationException = 10300;

        public const int TvHeadendEpgResetException = 10400;

        #endregion
    }
}
