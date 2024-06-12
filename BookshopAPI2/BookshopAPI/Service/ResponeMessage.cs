namespace BookshopAPI.Service
{
    public class ResponeMessage
    {
        public bool status {  get; set; }
        public string message { get; set; }
        public Object data {  get; set; }
        public ResponeMessage response500(Object? data)
        {
            return new ResponeMessage
            {
                status = false,
                message = "Có lỗi từ server, vui lòng thử lại sau",
                data = data
            };
        }
        public ResponeMessage response200(Object? data)
        {
            return new ResponeMessage
            {
                status = true,
                message = "Success",
                data = data
            };
        }
        public ResponeMessage response400(Object? data)
        {
            return new ResponeMessage
            {
                status = false,
                message = "Bad request",
                data = data
            };
        }

        public ResponeMessage response404(Object? data)
        {
            return new ResponeMessage
            {
                status = false,
                message = "Not found",
                data = data
            };
        }
    }
}
