namespace BookStoreUI.Static
{
    public class Endpoints
    {
        public static string BaseUrl = "https://localhost:44383/";
        public static string AuthorsEnpoint = $"{BaseUrl}api/authors/";
        public static string BooksEnpoint = $"{BaseUrl}api/books/";
        public static string RegisterEndpoint = $"{BaseUrl}api/users/register/";
        public static string LoginEndpoint = $"{BaseUrl}api/users/login/";
    }
}
