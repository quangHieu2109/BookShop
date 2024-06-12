namespace BookshopAPI.Service
{
    public class ResponeMessage
    {
        public bool status {  get; set; }
        public string message { get; set; }
        public Object data {  get; set; }
        public ResponeMessage response500(Object? data, string? msg = "Có lỗi từ server, vui lòng thử lại sau")
        {
            return new ResponeMessage
            {
                status = false,
                message = msg,
                data = data
            };
        }
        public ResponeMessage response200(Object? data, string? msg = "Success")
        {
            return new ResponeMessage
            {
                status = true,
                message = msg,
                data = data
            };
        }
        public ResponeMessage response400(Object? data, string? msg = "Bad request")
        {
            return new ResponeMessage
            {
                status = false,
                message = msg,
                data = data
            };
        }

        public ResponeMessage response404(Object? data, string? msg = "Not found")
        {
            return new ResponeMessage
            {
                status = false,
                message = msg,
                data = data
            };
        }
    }
}
