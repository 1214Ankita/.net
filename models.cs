// src/app/models/dish.model.ts
export abstract class Dish {
    id: number;
    name: string;
    description: string;
    price: number;
    type: string; // Dish type

    constructor(id: number, name: string, description: string, price: number) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.price = price;
    }

    abstract getDishType(): string; // Abstract method
}




// src/app/models/main-dish.model.ts
import { Dish } from './dish.model';

export class MainDish extends Dish {
    mainIngredient: string;

    constructor(id: number, name: string, description: string, price: number, mainIngredient: string) {
        super(id, name, description, price);
        this.mainIngredient = mainIngredient;
        this.type = 'MainDish'; // Set type
    }

    getDishType(): string {
        return 'Main Dish';
    }
}





// src/app/models/dessert.model.ts
import { Dish } from './dish.model';

export class Dessert extends Dish {
    isGlutenFree: boolean;

    constructor(id: number, name: string, description: string, price: number, isGlutenFree: boolean) {
        super(id, name, description, price);
        this.isGlutenFree = isGlutenFree;
        this.type = 'Dessert'; // Set type
    }

    getDishType(): string {
        return 'Dessert';
    }
}
