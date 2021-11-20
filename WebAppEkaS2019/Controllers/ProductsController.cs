using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAppEkaS2019.Models;
using System.Web.Mvc;
using PagedList;

namespace WebAppEkaS2019.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index(string sortOrder, string currentFilter1, string searchString1, string ProductCategory, string currentProductCategory, int? page, int? pagesize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ProductNameSortParm = String.IsNullOrEmpty(sortOrder) ? "productname_desc" : "";
            ViewBag.UnitPriceSortParm = sortOrder == "UnitPrice" ? "UnitPrice_desc" : "UnitPrice";
            //Hakufiltterin laitto muistiin
            if (searchString1 != null)
            {
                page = 1;
            }
            else
            {
                searchString1 = currentFilter1;
            }

            ViewBag.currentFilter1 = searchString1;

            //Tuotecategoriahakufiltterin laitto muistiin
            if ((ProductCategory != null) && (ProductCategory != "0"))
            {
                page = 1;
            }
            else
            {
                ProductCategory = currentProductCategory;
            }

            ViewBag.currentProductCategory = ProductCategory;

            northwindEntities db = new northwindEntities();

            var tuotteet = from p in db.Products
                           select p;

            //Tehdään tässä kohden haku tuoteryhmällä, jos se on asetettu käyttöliittymässä <-- seuraavat haut tarkentavat tätä tulosjoukkoa
            if (!String.IsNullOrEmpty(ProductCategory) && (ProductCategory != "0"))
            {
                int para = int.Parse(ProductCategory);
                tuotteet = tuotteet.Where(p => p.CategoryID == para);
            }

            if (!String.IsNullOrEmpty(searchString1)) //Jos hakufiltteri on käytössä, niin käytetään sitä ja sen lisäksi lajitellaan tulokset 
            {
                switch (sortOrder)
                {
                    case "productname_desc":
                        tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1)).OrderByDescending(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1)).OrderBy(p => p.UnitPrice);
                        break;
                    case "UnitPrice_desc":
                        tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1)).OrderByDescending(p => p.UnitPrice);
                        break;
                    default:
                        tuotteet = tuotteet.Where(p => p.ProductName.Contains(searchString1)).OrderBy(p => p.ProductName);
                        break;
                }
            }
            else if (!String.IsNullOrEmpty(ProductCategory) && (ProductCategory != "0")) //Jos käytössä on tuoteryhmärajaus, niin käytetään sitä ja sen lisäksi lajitellaan tulokset 
            {
                int para = int.Parse(ProductCategory);
                switch (sortOrder)
                {
                    case "productname_desc":
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderByDescending(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderBy(p => p.UnitPrice);
                        break;
                    case "UnitPrice_desc":
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderByDescending(p => p.UnitPrice);
                        break;
                    default:
                        tuotteet = tuotteet.Where(p => p.CategoryID == para).OrderBy(p => p.ProductName);
                        break;
                }
            } else { //Tässä hakufiltteri EI OLE käytössä, joten lajitellaan koko tulosjoukko ilman suodatuksia 
                switch (sortOrder)
                {
                    case "productname_desc":
                        tuotteet = tuotteet.OrderByDescending(p => p.ProductName);
                        break;
                    case "UnitPrice":
                        tuotteet = tuotteet.OrderBy(p => p.UnitPrice);
                        break;
                    case "UnitPrice_desc":
                        tuotteet = tuotteet.OrderByDescending(p => p.UnitPrice);
                        break;
                    default:
                        tuotteet = tuotteet.OrderBy(p => p.ProductName);
                        break;
                }
            };

            List<Categories> lstCategories = new List<Categories>();

            var categoryList = from cat in db.Categories
                               select cat;

            Categories tyhjaCategory = new Categories();
            tyhjaCategory.CategoryID = 0;
            tyhjaCategory.CategoryName = "";
            tyhjaCategory.CategoryIDCategoryName = "";
            lstCategories.Add(tyhjaCategory);

            foreach (Categories category in categoryList)
            {
                Categories yksiCategory = new Categories();
                yksiCategory.CategoryID = category.CategoryID;
                yksiCategory.CategoryName = category.CategoryName;
                yksiCategory.CategoryIDCategoryName = category.CategoryID.ToString() + " - " + category.CategoryName;
                //Taulun luokkamääritykseen Models-kansiossa piti lisätä tämä "uusi" kenttä = CategoryIDCategoryName
                lstCategories.Add(yksiCategory);
            }
            ViewBag.CategoryID = new SelectList(lstCategories, "CategoryID", "CategoryIDCategoryName", ProductCategory);

            int pageSize = (pagesize ?? 10); //Tämä palauttaa sivukoon taikka jos pagesize on null, niin palauttaa koon 10 riviä per sivu
            int pageNumber = (page ?? 1); //Tämä palauttaa sivunumeron taikka jos page on null, niin palauttaa numeron yksi
            return View(tuotteet.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ProdCards()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("MustAuthorize", "home");
            }
            else
            {
                northwindEntities db = new northwindEntities();
                List<Products> tuotteet = db.Products.ToList();
                db.Dispose();
                return View(tuotteet);
            }

        }

    }
}