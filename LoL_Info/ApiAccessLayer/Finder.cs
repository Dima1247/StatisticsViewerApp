using System;

namespace ApiAccessLayer
{
    public class Finder<Type>
    {
        public Finder(ref Type finalInfo, string path)
        {
            finalInfo = new ApiInformator<Type>(path).GetInfoInType();
        }
    }
}
