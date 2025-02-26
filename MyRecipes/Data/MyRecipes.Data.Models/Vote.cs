﻿using MyRecipes.Data.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyRecipes.Data.Models
{
    public class Vote : BaseModel<int>
    {
        public int RecipeId { get; set; }

        public Recipe Recipe { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public byte Value { get; set; }

    }
}
