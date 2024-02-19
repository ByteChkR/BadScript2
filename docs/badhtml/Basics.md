# Basic Usage

The template engine is completely html compatible. So the simplest template is just a plain HTML file.

```html
<!DOCTYPE html>
<html>
	<head>
		<meta charset="utf-8">
		<meta name="viewport" content="width=device-width, initial-scale=1">
		<title></title>
	</head>
	<body>

	</body>
</html>
```

> To be able to correctly generate PDF Files the template needs to define a complete HTML layout(as seen above)
> For 'pure' HTML templates it is not needed, but it is strongly recommended to do it anyway.

## Content Injection

Injecting Content into the HTML DOM is done via the `{}` syntax.

### Input
```html
<h1>{Model.Common.CompanyName}</h1>
```

### Output
```html
<h1>COMPANY_NAME</h1>
```

## Scripting

It is possible to define scripts in plain BadScript2 Syntax.

### Input
```html
<!-- Note: the attribute 'lang' must contain 'bs2' to get executed by the engine. If this is not the case, the script block will get copied to the result. -->
<script lang="bs2">
	let counter = 0;
	function GetCount()
	{
		return ++counter;
	}
</script>
<ul>
	<li>{GetCounter()}</li>
	<li>{GetCounter()}</li>
</ul>
```

### Output
```html
<ul>
	<li>1</li>
	<li>2</li>
</ul>
```

## Branching

The template supports branching by using a special `bs:if` tag or the `if` keyword in plain BadScript2 Syntax.

### Input
```html
<script lang="bs2">
	if(Model.Data.MyValue != "THIS IS A TEXT") //Normal Branches as in Javascript
	{

	}
</script>

<div>
	<bs:if test="Model.Data.MyBoolean">
		<!-- Gets evaluated if Model.Data.MyBoolean equals 'true' -->
	<bs:else/>
		<!-- Gets evaluated if Model.Data.MyBoolean equals 'false' -->
	</bs:if>

	<bs:if test="Model.Data.MyNumber > 0">
		<!-- Gets evaluated if Model.Data.MyNumber is greater than '0' -->
	<bs:else test="Model.Data.MyNumber < 0"/>
		<!-- Gets evaluated if Model.Data.MyNumber is smaller than '0' -->
	<bs:else/>
		<!-- Gets evaluated if Model.Data.MyNumber equals '0' -->
	</bs:if>
</div>
```

### Output

```html
<div>
		<!-- Gets evaluated if Model.Data.MyBoolean equals 'true' -->
		<!-- Gets evaluated if Model.Data.MyNumber equals '0' -->
</div>
```

Assuming the Data Table looks like:
```json
{
	"MyBoolean": true,
	"MyNumber": 0
}
```

## Loops

The template engine supports loop blocks whose content will be repeatedly evaluated.

### For-Each Loop

The `bs:each` block can be used to iterate over an array-like structure.

#### Input
```html
<ul>
	<bs:each on="Model.Data.MyArray" as="item">
		<li><a href="{item.Url}">{item.Name}</a></li>
	</bs:each>
</ul>
```

#### Output
```html
<ul>
		<li><a href="https://google.de">Google Deutschland</a></li>
		<li><a href="https://ewu-it.de">EWU-IT GmbH</a></li>
</ul>
```

Assuming the Data Table looks like:
```json
{
	"MyArray": [
		{
			"Name": "Google Deutschland",
			"Url": "https://google.de"
		},
		{
			"Name": "EWU-IT GmbH",
			"Url": "https://ewu-it.de"
		}
	]
}
```

### While Loop

The `bs:while` block can be used to iterate until a specified condition returns true.
> This is dangerous because of possible infinite loops.

#### Input

```html
<script lang="bs2">
	const array = [
		"A",
		"B",
		"C"
	]

	//Use the enumerator explicitly
	//This is what happens more efficiently in the BadScript2 Language Runtime when using a foreach loop or the bs:each block
	const enumerator = array.GetEnumerator(); 
</script>
<div>
	<bs:while test="enumerator.MoveNext()">
		<p>{enumerator.GetCurrent()}</p>
	</bs:while>
</div>
```
> There is little to no reason to use a `bs:while` block over a `bs:each`. It will lead to problems unless you know exactly what you are doing.

#### Output
```html
<div>
		<p>A</p>
		<p>B</p>
		<p>C</p>
</div>
```

## Best Practices

Since BadScript2 is a multipurpose language it is possible to achieve the goal in many different ways, in this section it will be outlined what are the DOs and the DONTs are.

### Prefer `bs:each` over `bs:while` wherever possible

`bs:each` is faster and safer than `bs:while` because for-each loops be optimized by the language runtime and are generally a safer concept for iterating. Since in the context of template evaluation a looping block is almost always used to emit multiple html elements with different data, it is rarely beneficial to use a `bs:while` block.

### Placement of script blocks and template functions

Since BadScript2 is an interpreted language, script blocks and functions will be created and evaluated multiple times if placed within a for loop.
While this may not make a measurable difference if the iterations are small, the execution time will grow massively when a large amount of data is involved.

### Encapsulate frequently used template parts

If a template requires the same snippet of template code to be injected at multiple places, use the `bs:function` tag to be able to reduce duplication

### Do not Modify the Model within the template

While its possible to modify (and even delete) the `Model` object from the runtime context, it will lead to unexpected behaviour and hard to detect bugs. Do this only when you absolutely need to persist data from other executed templates.
> The Template Service will evaluate the templates in the following order: Header, Footer, Template

## Limitations

As the idea to use BadScript2 as a template engine was not the initial goal, there are some limitations that can unfortunately not be resolved.


### Using strings in attribute evaluations

Because there is some overlap in the html and bs2 syntax it is impossible to use strings directly inside attributes.

```html
<bs:if test="Model.Data.MyString == "ABC""> <!-- This is invalid HTMl and will not be parsed correctly. -->
	<!-- DO THE THING -->
</bs:if>
```

> To still use strings inside attributes, use the single quote symbol. `"ABC" => 'ABC'`

However it is possible to use strings in the `{}` substitutions __*outside*__ of attributes

```html
<div>
	{Model.Data.MyString ?? "ABC"}
</div>
```


#### The workaround

Declare the string inside a script block
```html
<script lang="bs2">
	const ABC = "ABC";
</script>
<bs:if test="Model.Data.MyString == ABC">
	<!-- DO THE THING -->
</bs:if>
```

### `await` keyword does not work

The BadScript2 language runtime has a Asynchronous Scheduling System that is disabled for templates.
> This is a limitation that __*can*__ be resolved with some time-investment but was deemed not worth the time at the point of writing.

### `style` and `script` tags

Because the language runtime has a large overlap of used symbols with CSS and JavaScript, it is not possible to use substitution within `script` or `style` tags.
The tags get copied to the output without evaluating the inner content.

##### The workaround

Construct the css/javscript inside a `bs2` script block

```html

<script lang="bs2">
	
    //This is a hack for github pages, because it uses the same syntax as the template engine
    const doubleOpen = "{"+"{";
    const doubleClose = "}"+"}";

	const color = "#007daa"
	//Multi-line Format Strings(starting with $@ need to escape the { and } characters by using them twice) 
	const css = $@"
		<style>
			.test {doubleOpen}
				background-color: {color};
            {doubleClose}
		</style>
	";

	const text = "\"Hello World!\"";
	const script = $@"
		<script>
			function onClick = () => alert({text});
		</script>
	";

</script>

{css}
{script}

<div class="test" onclick="onClick"></div>

```

___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)