# vueapp Default Readme Elements

## Project setup
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Compiles and minifies for production
```
npm run build
```

### Lints and fixes files
```
npm run lint
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).

# Jsonify Readme

## Purpose
Present BAs and other users with a way to visualize Assurity contracts and generate valid JSON for them.

## Design
Relies on recursion, nesting, and prop mutation. Prop mutation is controversial and often considered an "anti-pattern"
for Vue, React, Angular, etc. In this case, the app is built around prop mutation, as this is the most direct and efficient way
to accomplish the goal. This means that there is no unnecessary duplication, copying, deleting, replacing, and so forth of 
elements. We have one copy of everything. Obviously if there were downstream presentation/data processing activities, this would
be problematic. But there are not.

As a result, I have disabled the `unexpected mutation of <prop name here> prop` rule from the linter.

### How It Works
1. Users must either select a service, a product, or both.
2. Once selected, a list of objects is presented with their full namespace.
3. Choose an object and it will load and generate default values for individual fields.
4. Once an object is chosen, data is passed to DataMaster. DataMaster converts to a different object, which is populated with default values, then passed to JsonView and DataDisplay.
5. DataDisplay provides the visual and interactive elements for data manipulation.
6. JsonView converts to JSON and displays as text.

#### Default Values
- Unless there are special default value rules, default values for all string fields is the field name itself.
- For decimal and int, we default to 0.0 and 0.
- Some fields have pre-determined default values that are intended to help avoid common validation rules.
    - E.g., phone number, social security number, etc
- Except for `List` property types, default values (and manipulated values) are stored in `setValue`.
- For `List` properties, we use the `properties` as a "template" for all JSON arrays that the user generates.
    - This `properties` field is never altered.
    - Actual manipulated/generated lists are stored in `collections` field.
- Default values are also set in the `defaultValue` field, so that we can restore a value when field is set to `null`.

#### Lists
When a property type is `List`, data handling is very different.
- The `arraySize` field is used to determine how many entries there are for a given array.
- The `properties` field serves as a "template" for each array that gets generated. It is not manipulated directly.
- Each generated array is stored in the `collections` field.
- A dedicated `ListData` component exists to handle these interactions.

## Future Considerations
- Example support
    - We have so many examples in solution. Is it possible to allow users to select those?

- Upload JSON
    - Once a user has selected an object, is it possible for us to allow them to upload a pre-populated JSON example?

- I chose Element Plus as a Vue "template" since it offered a really nice DateTime picker and seemed easy to pick up. 
    - But is my use of their cards for displaying information (specifically, to display nesting of objects) the most efficient?
Maybe not. Initial load can take a second or two for large objects. There may be simpler ways of presenting info that are
quicker. I have not noticed any performance issues once the contract is loaded, though.

- "Copy to Clipboard" button
    - Maybe this should have a static position somewhere? It's annoying to scroll down, update a field, scroll up, copy, etc.

- Color Theme
    - This could probably be improved.
    - Boolean fields can be pretty ugly depending on the depth of the element.

- Better / More Useful default JSON values
