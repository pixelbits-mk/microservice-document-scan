using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanner.Validation.Models
{
    public class ValidationContext<TModel, TContext>
    {
        public TModel Model { get; set; }
        public TContext Context { get; set; }

        public ValidationContext(TModel model, TContext context)
        {
            Model = model;
            Context = context;
        }
    }
}
