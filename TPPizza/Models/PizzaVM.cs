using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TPPizza;

public class PizzaVM
{
        public int Id { get; set; }

        [Required(ErrorMessage = "Donnez un nom à votre pizza !")]
        [StringLength(20, ErrorMessage = "Le nom de votre pizza doit être compris entre {2} et {1} caractères", MinimumLength = 5)]
        public string? Nom { get; set; }

        [DisplayName("Pâte")]
        [Required(ErrorMessage = "Une pizza sans pâte ça n'existe pas !")]
        public int IdPate { get; set; }

        public SelectList ChoixPate => new SelectList(Pizza.PatesDisponibles, "Id", "Nom");

        [DisplayName("Ingrédients")]
        [Required(ErrorMessage = "Une pizza sans ingrédients ? Vous n'avez pas de goût ?")]
        public List<int>? IdsIngredients { get; set; }

        public SelectList ChoixIngredients => new SelectList(Pizza.IngredientsDisponibles, "Id", "Nom");
}
