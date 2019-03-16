namespace SSLLabsApiWrapper.Models.Response
{
	public class StatusCodes : BaseModel
	{
		public StatusDetails StatusDetails { get; set; }

		public StatusCodes()
		{
			StatusDetails = new StatusDetails();
		}
			
	}
}
