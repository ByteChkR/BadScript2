# Advanced Scripting

While the Basics section is enough to get started, there are more features that are not needed but can be very useful to know about.

## Template Functions

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

## Template Injection Tricks

### Tip 1: Cache Template invocations

#### Wrong
```html

<bs:each on="Model.Data.MyArray" as="item">
	<bs:template path="./myTemplate.bhtml" model="{Model}"/>
	...
</bs:each>

```

#### Correct
```html

<script lang="bs2">
	
	const template = BadHtml.Run("./myTemplate.bhtml", Model);

</script>

<bs:each on="Model.Data.MyArray" as="item">
	{template}
	...
</bs:each>

```

### Tip 2: Injecting custom content into Templates

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

## Debugging

With BadScript2 comes a debugger that can also be used to debug template files.

When an Expression is executed, the debugger will halt execution once the execution is passed to the template engine.

Every time a new file is loaded the debugger will ask if this file needs to be debugged.(default is: no)

> DO NOT LOAD THE DEBUGGER FILE INTO THE DEBUGGER. It will make the Runtime Stackoverflow
The debugger will start in `step mode` by default. This allows for stepping through the code line by line.

### Commands

The debugger has multiple commands that can help debugging.

#### `help`
Displays all available commands that are registered.

#### `step`
Toggles step mode. If step mode is deactivated the debugger will execute unless a breakpoint is hit.

#### `lbp`
Lists all active breakpoints.

#### `view`
Shows the sourceview of the debugged file.

#### `eval`
Evaluates a piece of BadScript2 inside the execution context of the running script.
> `eval` expects the code to end with a semicolon.
> Example: `eval Model;` will return the "JSON-ified" result of the Model object.

#### `sbp`
Sets a Breakpoint to the specified file/line with the specified condition

##### Arguments
- `sbp` with no arguments will set a line breakpoint with no condition in the current file at the current line.
- `sbp <linenr>` will set a line breakpoint with no condition in the current file at the specified line.
- `sbp <file> <linenr>` will set a line breakpoint with no condition in the specified file and line.
- `sbp <file> <linenr> <condition>` will set a line breakpoint at the specified file and line that only triggers if the condition specified evaluates to `true`.


## Advanced Injecting

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

### Limitations

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
___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)