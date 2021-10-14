# CosmosDbExperiments

## Using global distribution

### Querying without replication to nearest region

### Querying with replication to nearest region enabled

### Writing documents without multi-write regions enabled

### Writing documents with multi-write regions enabled

<hr>

## Fine-tuning indexing

### Querying with default indexing settings - indexing all fields

### Querying after fine-tuning indexing settings - only required fields are indexed

<hr>

## To run this experiments on your own -

<ul>
    <li>Make Sure Dotnetcore 3.1 is installed on your system</li>
    <li>Create an Cosmos DB account in Azure and update <em>endpoint</em>, <em>region</em> and <em>primaryKey</em>
        in CosmosAccountSettings section of appsettings.json accordingly.
    </li>
</ul>