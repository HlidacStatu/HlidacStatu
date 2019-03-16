using System;

namespace SSLLabsApiWrapper.Domain
{
	class UrlValidation
	{
		public bool IsValid(string url)
		{
			var valid = true;

			Uri uri = null;
			if (!Uri.TryCreate(url, UriKind.Absolute, out uri) || null == uri)
			{
				valid = false;
			}

			return valid;
		}

		public string Format(string url)
		{
			if (!url.EndsWith("/"))
			{
				url = url + "/";
			}

			return url;
		}
	}
}