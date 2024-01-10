using System;
namespace AuthWeb.Models
{
	public record struct CreateAnimalDto(
       
        string Name,
		string Age,
		string ExtraExplanations,
		string imagePath,
        string userName,
        string userPhone,
        List<String> Vaccines
		);
}

