# BadHtml Documentation

Language documentation for BadHtml

## Table of Contents

- [BadHtml Language Documentation](#badhtml-documentation)
    - [Table of Contents](#table-of-contents)
    - [Basic Usage](#basic-usage)
        - [Content Injection](#content-injection)
            - [Input](#input-1)
            - [Output](#output-1)
        - [Scripting](#scripting)
            - [Input](#input-2)
            - [Output](#output-2)
        - [Branching](#branching)
            - [Input](#input-3)
            - [Output](#output-3)
        - [Loops](#loops)
            - [For-Each Loop](#for-each-loop)
                - [Input](#input-4)
                - [Output](#output-4)
            - [While Loop](#while-loop)
                - [Input](#input-4)
                - [Output](#output-4)
        - [Best Practices](#best-practices)
            - [Prefer `bs:each` over `bs:while` wherever possible](#prefer-bseach-over-bswhile-wherever-possible)
            - [Placement of script blocks and template functions](#placement-of-script-blocks-and-template-functions)
            - [Encapsulate frequently used template parts](#encapsulate-frequently-used-template-parts)
            - [Do not Modify the Model within the template](#do-not-modify-the-model-within-the-template)
        - [Limitations](#limitations)
            - [Using strings in attribute evaluations](#using-strings-in-attribute-evaluations)
                - [The workaround](#the-workaround)
            - [`await` keyword does not work](#await-keyword-does-not-work)
            - [`style` and `script` tags](#style-and-script-tags)
                - [The workaround](#the-workaround-1)
    - [Advanced Scripting](#advanced-scripting)
        - [Template Functions](#template-functions)
        - [Template Injection Tricks](#template-injection-tricks)
            - [Tip 1: Cache Template invocations](#tip-1-cache-template-invocations)
            - [Tip 2: Injecting custom content into Templates](#tip-2-injecting-custom-content-into-templates)
        - [Debugging](#debugging)
            - [Commands](#commands)
                - [`help`](#help)
                - [`step`](#step)
                - [`lbp`](#lbp)
                - [`view`](#view)
                - [`eval`](#eval)
                - [`sbp`](#sbp)
                    - [Arguments](#arguments)
        - [Advanced Injecting](#advanced-injecting)
            - [Limitations](#limitations-1)

## Basic Usage

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

### Content Injection

Injecting Content into the HTML DOM is done via the `{}` syntax.

#### Input
```html
<h1>{Model.Common.CompanyName}</h1>
```

#### Output
```html
<h1>COMPANY_NAME</h1>
```

### Scripting

It is possible to define scripts in plain BadScript2 Syntax.

#### Input
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

#### Output
```html
<ul>
	<li>1</li>
	<li>2</li>
</ul>
```

### Branching

The template supports branching by using a special `bs:if` tag or the `if` keyword in plain BadScript2 Syntax.

#### Input
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

#### Output

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

### Loops

The template engine supports loop blocks whose content will be repeatedly evaluated.

#### For-Each Loop

The `bs:each` block can be used to iterate over an array-like structure.

##### Input
```html
<ul>
	<bs:each on="Model.Data.MyArray" as="item">
		<li><a href="{item.Url}">{item.Name}</a></li>
	</bs:each>
</ul>
```

##### Output
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

The Loop block can also define the Index as a variable by using the `index` attribute.

```html
<bs:each on="Model.Data.MyArray" as="item" index="i">
    <li><a href="{item.Url}">{i} {item.Name}</a></li>
</bs:each>
```

#### While Loop

The `bs:while` block can be used to iterate until a specified condition returns true.
> This is dangerous because of possible infinite loops.

##### Input

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

##### Output
```html
<div>
		<p>A</p>
		<p>B</p>
		<p>C</p>
</div>
```

### Best Practices

Since BadScript2 is a multipurpose language it is possible to achieve the goal in many different ways, in this section it will be outlined what are the DOs and the DONTs are.

#### Prefer `bs:each` over `bs:while` wherever possible

`bs:each` is faster and safer than `bs:while` because for-each loops be optimized by the language runtime and are generally a safer concept for iterating. Since in the context of template evaluation a looping block is almost always used to emit multiple html elements with different data, it is rarely beneficial to use a `bs:while` block.

#### Placement of script blocks and template functions

Since BadScript2 is an interpreted language, script blocks and functions will be created and evaluated multiple times if placed within a for loop.
While this may not make a measurable difference if the iterations are small, the execution time will grow massively when a large amount of data is involved.

#### Encapsulate frequently used template parts

If a template requires the same snippet of template code to be injected at multiple places, use the `bs:function` tag to be able to reduce duplication

#### Do not Modify the Model within the template

While its possible to modify (and even delete) the `Model` object from the runtime context, it will lead to unexpected behaviour and hard to detect bugs. Do this only when you absolutely need to persist data from other executed templates.
> The Template Service will evaluate the templates in the following order: Header, Footer, Template

### Limitations

As the idea to use BadScript2 as a template engine was not the initial goal, there are some limitations that can unfortunately not be resolved.


#### Using strings in attribute evaluations

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


##### The workaround

Declare the string inside a script block
```html
<script lang="bs2">
	const ABC = "ABC";
</script>
<bs:if test="Model.Data.MyString == ABC">
	<!-- DO THE THING -->
</bs:if>
```

#### `await` keyword does not work

The BadScript2 language runtime has a Asynchronous Scheduling System that is disabled for templates.
> This is a limitation that __*can*__ be resolved with some time-investment but was deemed not worth the time at the point of writing.

#### `style` and `script` tags

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

## Advanced Scripting

While the Basics section is enough to get started, there are more features that are not needed but can be very useful to know about.

### Template Functions

Template functions are a feature of the Template Engine. It allows a template to declare a block of html elements that can be invoked like any 'normal' function.

```html

<bs:function name="DisplayItem">
	<p>Hello World!</p>
</bs:function>

<div>
	{DisplayItem()} <!-- Injects the p tag defined in the function into the div -->
</div>

```

```html

<bs:function name="DisplayItem" param:text> <!-- This function defines a parameter for the function -->
	<p>{text}</p>
</bs:function>

<div>
	{Display("Hello World")} <!-- Injects the p tag with the "Hello World" text inside it -->
</div>

```

BadScript2 Supports dynamic type checking and so does the template engine
```html
<bs:function name="DisplayItem" param:text="string"> <!-- The Language runtime checks the type of the parameter but also allows null to be passed. -->
	<p>{text}</p>
</bs:function>

{DisplayItem("Hello World")}

<bs:function name="DisplayItemOptional" param:text="string?">
	<!-- The Language runtime checks the type of the parameter but also allows no parameter to be passed to the function-->
	<p>{text ?? "Hello World"}</p>
</bs:function>

{DisplayItemOptional("Hello World")}
{DisplayItemOptional()}

<bs:function name="DisplayItemNotNull" param:text="string!">
	<!-- The Language runtime checks the type of the parameter but does not allow null to be passed. -->
	<p>{text ?? "Hello World"}</p>
</bs:function>

<bs:function name="DisplayItemRestArgs" param:first param:args="*">
	<!-- using the * modifier in argument types the parameter will contain all remaining arguments that can not be mapped to the other defined parameters -->
	<p>{first}</p>
	<bs:each on="args" as="arg">
		<p>{arg}</p>
	</bs:each>
</bs:function>
<!-- the args parameter will be an array with a single element "Hallo Welt" when invoked like this -->
{DisplayItemRestArgs("Hello World", "Hallo Welt")}

```

### Template Injection Tricks

#### Tip 1: Cache Template invocations

##### Wrong
```html

<bs:each on="Model.Data.MyArray" as="item">
	<bs:template path="./myTemplate.bhtml" model="{Model}"/>
	...
</bs:each>

```

##### Correct
```html

<script lang="bs2">
	
	const template = BadHtml.Run("./myTemplate.bhtml", Model);

</script>

<bs:each on="Model.Data.MyArray" as="item">
	{template}
	...
</bs:each>

```

#### Tip 2: Injecting custom content into Templates

It is not possible to use something like Sveltes `<slot/>` system to inject content, however it is possible to inject template functions into other templates.

```html
<bs:function name="Wrapper1" param:content>
	<div style="width: 100%; height: 100%"> <!-- Does some styling -->
		{content} <!-- Inject content directly -->
	</div>
</bs:function>

<bs:function name="ContentFunc" param:text>
	<p>{text}</p>
</bs:function>

<bs:function name="Wrapper2" param:func>
	<div style="width: 100%; height: 100%"> <!-- Does some styling -->
		{func("Hello World!")} <!-- Call Content Function with parameter -->
	</div>
</bs:function>

{Wrapper1(ContentFunc("Hello World!"))} <!-- Calls the specified wrapper function with the already evaluated Content. -->
{Wrapper2(ContentFunc)} <!-- Calls the specified wrapper function with the Content function hat gets called within the wrapper. -->
```

### Debugging

With BadScript2 comes a debugger that can also be used to debug template files.

When an Expression is executed, the debugger will halt execution once the execution is passed to the template engine.

Every time a new file is loaded the debugger will ask if this file needs to be debugged.(default is: no)

> DO NOT LOAD THE DEBUGGER FILE INTO THE DEBUGGER. It will make the Runtime Stackoverflow
The debugger will start in `step mode` by default. This allows for stepping through the code line by line.

#### Commands

The debugger has multiple commands that can help debugging.

##### `help`
Displays all available commands that are registered.

##### `step`
Toggles step mode. If step mode is deactivated the debugger will execute unless a breakpoint is hit.

##### `lbp`
Lists all active breakpoints.

##### `view`
Shows the sourceview of the debugged file.

##### `eval`
Evaluates a piece of BadScript2 inside the execution context of the running script.
> `eval` expects the code to end with a semicolon.
> Example: `eval Model;` will return the "JSON-ified" result of the Model object.

##### `sbp`
Sets a Breakpoint to the specified file/line with the specified condition

###### Arguments
- `sbp` with no arguments will set a line breakpoint with no condition in the current file at the current line.
- `sbp <linenr>` will set a line breakpoint with no condition in the current file at the specified line.
- `sbp <file> <linenr>` will set a line breakpoint with no condition in the specified file and line.
- `sbp <file> <linenr> <condition>` will set a line breakpoint at the specified file and line that only triggers if the condition specified evaluates to `true`.


### Advanced Injecting

It is possible to inject content into other elements than the current one with `bs:insert`

```html
<h1>TEST</h1>
<div>
	<ul id="table-of-content"> <!-- The Element to insert into -->
		
	</ul>
</div>

<script lang="bs2">
	
	const elements = [{title: "A", text: "ABC"}, {title: "B", text: "ABC"}, {title: "C", text: "ABC"}];

</script>

<bs:each on="elements" as="item">

	<bs:insert into="#table-of-content"> <!-- Insert into elemtent with ID: table-of-content -->
		<li>{item.title}</li>
	</bs:insert>
	<div>
		<h1>{item.title}</h1>
		<p>{item.text}</p>
	</div>

</bs:each>
```

#### Limitations

Injecting elements based on path only works if the destination element is in the same context as the `bs:insert` block.

> This will not work because a `bs:function` is evaluated with its own output document.

```html
<h1>TEST</h1>
<div>
	<ul id="table-of-content">
		
	</ul>
</div>

<script lang="bs2">
	
	const elements = [{title: "A", text: "ABC"}, {title: "B", text: "ABC"}, {title: "C", text: "ABC"}];

</script>

<bs:function name="CreateElement" param:item>
	<bs:insert into="#table-of-content">
		<li>{item.title}</li>
	</bs:insert>
	<div>
		<h1>{item.title}</h1>
		<p>{item.text}</p>
	</div>
</bs:function>

<bs:each on="elements" as="item">
	{CreateElement(item)}	
</bs:each>
```
