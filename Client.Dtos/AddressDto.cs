namespace Client.Dtos;

public class AddressDto
{
    public string AddressLineOne { get; set; }
    public string? AddressLineTwo { get; set; }
    public string? AddressLineThree { get; set; }
    public string PostCode { get; set; }
}