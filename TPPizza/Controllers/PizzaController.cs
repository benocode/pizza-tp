using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace TPPizza;

public class PizzaController : Controller
{
    private static List<Pizza> _pizzas =
    [
        new() { Id = 1, Nom = "Regina",
            Pate = Pizza.PatesDisponibles[0],
            Ingredients = [
                Pizza.IngredientsDisponibles[0],
                Pizza.IngredientsDisponibles[1],
                Pizza.IngredientsDisponibles[2],
                Pizza.IngredientsDisponibles[6]
            ]
        },
        new() { Id = 2, Nom = "Chicken",
            Pate = Pizza.PatesDisponibles[1],
            Ingredients = [
                Pizza.IngredientsDisponibles[2],
                Pizza.IngredientsDisponibles[4],
                Pizza.IngredientsDisponibles[7],
            ]
        }
    ];

    // GET: PizzaController
    public IActionResult Index()
    {
        return View(_pizzas);
    }

    // GET: PizzaController/Details/5
    public IActionResult Details(int id)
    {
        Pizza? pizza = _pizzas.Find(p => p.Id == id);
        if (pizza is null)
        {
            return NotFound();
        }
        return View(pizza);
    }

    // GET: PizzaController/Create
    public IActionResult Create()
    {
        return View(new PizzaVM());
    }

    // POST: PizzaController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(PizzaVM pizzaVM)
    {
        try
        {
            if(!IsValid(pizzaVM))
            {
                return View(pizzaVM);
            }
            int idPizza = _pizzas.Max(p => p.Id) + 1;
            Pate pate = Pizza.PatesDisponibles.First(p => p.Id == pizzaVM.IdPate);
            List<Ingredient> ingredients = new List<Ingredient>();
            if (pizzaVM.IdsIngredients!.Count > 0) {
                foreach (var idIngredient in pizzaVM.IdsIngredients) {
                    ingredients.Add(Pizza.IngredientsDisponibles.First(i => i.Id == idIngredient));
                }
            }
            _pizzas.Add(new Pizza {
                Id = idPizza,
                Nom = pizzaVM.Nom!.Trim(),
                Pate = pate,
                Ingredients = ingredients
            });
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View(pizzaVM);
        }
    }

    // GET: PizzaController/Edit/5
    public IActionResult Edit(int id)
    {
        Pizza? pizza = _pizzas.Find(p => p.Id == id);
        if (pizza is null)
        {
            return NotFound();
        }
        return View(new PizzaVM {
            Id = pizza.Id,
            Nom = pizza.Nom,
            IdPate = pizza.Pate!.Id,
            IdsIngredients = pizza.Ingredients.Select(i => i.Id).ToList()
        });
    }

    // POST: PizzaController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(PizzaVM pizzaModifiee)
    {
        try
        {
            if(!IsValid(pizzaModifiee))
            {
                return View(pizzaModifiee);
            }
            Pizza? pizzaAModifier = _pizzas.Find(p => p.Id == pizzaModifiee.Id);
            if (pizzaAModifier is null)
            {
                return NotFound();
            }
            pizzaAModifier.Nom = pizzaModifiee.Nom!.Trim();
            pizzaAModifier.Pate = Pizza.PatesDisponibles.First(p => p.Id == pizzaModifiee.IdPate);
            pizzaAModifier.Ingredients = Pizza.IngredientsDisponibles.Where(i => pizzaModifiee.IdsIngredients!.Contains(i.Id)).ToList();
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View(pizzaModifiee);
        }
    }

    // GET: PizzaController/Delete/5
    public IActionResult Delete(int id)
    {
        Pizza? pizza = _pizzas.Find(p => p.Id == id);
        if (pizza is null)
        {
            return NotFound();
        }
        return View(pizza);
    }

    // POST: PizzaController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, IFormCollection collection)
    {
        try
        {
            Pizza? pizza = _pizzas.Find(p => p.Id == id);
            if (pizza is null)
            {
                return NotFound();
            }
            else
            {
                _pizzas.Remove(pizza);
            }
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }

    private bool IsValid(PizzaVM pizza)
    {
        if (!ModelState.IsValid)
            return false;
        if (NameAlreadyExists(pizza.Nom!, pizza.Id))
        {
            ModelState.AddModelError("Nom", "Le nom de votre pizza doit être unique !");
            return false;
        }
        if (pizza.IdsIngredients!.Count < 2 || pizza.IdsIngredients!.Count > 5 )
        {
            ModelState.AddModelError("IdsIngredients", "Pour être bonne, une pizza doit contenir entre 2 et 5 ingrédients !");
            return false;
        }
        if (HasSameIngredients(pizza.IdsIngredients!, pizza.Id))
        {
            ModelState.AddModelError("IdsIngredients", "Votre pizza contient les mêmes ingrédients qu'une autre pizza !");
            return false;
        }
        return true;
    }

    private bool HasSameIngredients(List<int> idsIngredients, int id)
    {
        return _pizzas.Where(p => p.Id != id && p.Ingredients.Count() == idsIngredients.Count()).Any(p => p.Ingredients.All(i => idsIngredients.Contains(i.Id)));
    }

    private bool NameAlreadyExists(string nom, int id)
    {
        return _pizzas.Any(p => p.Nom!.ToLower().Equals(nom.Trim().ToLower()) && p.Id != id);
    }
}
