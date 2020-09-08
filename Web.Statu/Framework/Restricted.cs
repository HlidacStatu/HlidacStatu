using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace HlidacStatu.Web.Framework
{
    class DisposableHelper : IDisposable
    {
        private Action end;

        // When the object is created, write "begin" function
        public DisposableHelper(Action begin, Action end)
        {
            this.end = end;
            begin();
        }

        // When the object is disposed (end of using block), write "end" function
        public void Dispose()
        {
            end();
        }
    }
    public class Restricted : IDisposable
    {
        public bool Allow { get; set; }

        private StringBuilder _stringBuilderBackup;
        private StringBuilder _stringBuilder;
        private readonly HtmlHelper _htmlHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="Restricted"/> class.
        /// </summary>
        public Restricted(HtmlHelper htmlHelper, bool allow)
        {
            Allow = allow;
            _htmlHelper = htmlHelper;
            if (!allow) BackupCurrentContent();
        }

        private void BackupCurrentContent()
        {
            // make backup of current buffered content
            _stringBuilder = ((System.IO.StringWriter)_htmlHelper.ViewContext.Writer).GetStringBuilder();
            _stringBuilderBackup = new StringBuilder().Append(_stringBuilder);
        }

        private void DenyContent()
        {
            // restore buffered content backup (destroying any buffered content since Restricted object initialization)
            _stringBuilder.Length = 0;
            _stringBuilder.Append(_stringBuilderBackup);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!Allow)
                DenyContent();
        }
    }

}