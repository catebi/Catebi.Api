import os
import re

def sql_to_mermaid(file_path, schema_name):

    # Read SQL script from the file
    with open(file_path, 'r') as file:
        sql_script = file.read()

    # Escape special characters in schema name for regex
    escaped_schema_name = re.escape(schema_name)

    # Regex patterns to extract table, column, and foreign key information
    table_pattern = rf"create table {escaped_schema_name}\.(\w+)\s*\(([^;]+)\);"
    column_pattern = r"(\w+)\s+([\w\s]+)(?:,|$)"
    fk_pattern = rf"alter table {escaped_schema_name}\.(\w+)\s+add FOREIGN KEY \((\w+)\)\s+references {escaped_schema_name}\.(\w+)\((\w+)\);"

    # Find all table definitions in the SQL script
    tables = re.findall(table_pattern, sql_script, re.IGNORECASE)
    # Find all foreign key relationships
    foreign_keys = re.findall(fk_pattern, sql_script, re.IGNORECASE)

    # Start the Mermaid ER diagram
    mermaid_script = "erDiagram\n"

    # Process foreign keys
    fk_relationships = {}
    for fk_table, fk_column, ref_table, ref_column in foreign_keys:
        fk_table_name = fk_table.lower()
        ref_table_name = ref_table.lower()
        if fk_table_name not in fk_relationships:
            fk_relationships[fk_table_name] = []
        fk_relationships[fk_table_name].append((fk_column, ref_table_name, ref_column))

    # Process each table
    for table, columns in tables:
        table_name = table.lower()  # Convert to snake case and lower case
        # primary_keys = re.findall(pk_pattern, columns, re.IGNORECASE)

        mermaid_script += f"    {table_name} {{\n"

        # Extract and add column names and types
        column_defs = re.findall(column_pattern, columns)
        for col_name, col_type in column_defs:
            # pk_label = " PK" if col_name in primary_keys else ""
            pk_label = " PK" if "primary key" in col_type.lower() else ""
            fk_label = " FK" if any(fk for fk, ref, ref_col in fk_relationships.get(table_name, []) if fk == col_name) else ""
            not_null_unique = " nn" if "not null" in col_type.lower() else ""
            not_null_unique += " u" if "unique" in col_type.lower() else  ""
            not_null_unique = not_null_unique.strip()

            if (not_null_unique != ""):
                not_null_unique = f" \"{not_null_unique}\""

            col_type = col_type.strip().lower().replace("not null", "").replace("unique", "").strip()
            if ("serial" in col_type.lower()):
                col_type = "int"
            mermaid_script += f"        {col_name} {col_type}{pk_label}{fk_label}{not_null_unique}\n"

        mermaid_script += "    }\n"

    # Add relationships
    for fk_table, relations in fk_relationships.items():
        for fk_column, ref_table, ref_column in relations:
            # mermaid_script += f"    {fk_table} ||--o{{ {ref_table} : \"{fk_column} -> {ref_column}\"\n"
            mermaid_script += f"    {fk_table} ||--o{{ {ref_table} : \" \"\n"

    return mermaid_script

# Get the path of the current script
script_path = os.path.abspath(__file__)

# Get the directory containing the script
script_dir = os.path.dirname(script_path)

# File path of the SQL script
file_path = script_dir + "/../db/changelog/releases/1.0.0/03._medical_care_tables.sql"
# "/../db/changelog/releases/1.0.0/04._freegan_tables.sql"
# "/../db/changelog/releases/1.0.0/03._medical_care_tables.sql"
# print("file Path:", file_path)

# Convert SQL to Mermaid
schema_name = "ctb"
mermaid_script = sql_to_mermaid(file_path, schema_name)

# Print the Mermaid script
print(mermaid_script)
