﻿namespace Entities.DataTransferObjects
{

    //[Serializable]
    //  public record BookDto(int Id, String Title, decimal Price); 
    public record BookDto
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public decimal Price { get; set; }
    }

}
