using System;
using System.ComponentModel.DataAnnotations;

namespace AuthWeb.Models;

	public class CreatePost
	{
    [Key]
    public int AnimalId { get; set; }

    public String Name { get; set; }

    public String Type { get; set; }

    public int Age { get; set; }

    public String? ExtraExplanations { get; set; }

    public String? imagePath { get; set; }

    public List<String> animalVaccines { get; set; }

    public String UserPhone { get; set; }

    public String UserName { get; set; }

}

