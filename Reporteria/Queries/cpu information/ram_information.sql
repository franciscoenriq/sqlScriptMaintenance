SELECT 
    (physical_memory_in_use_kb / 1024) AS memory_in_use_mb,
    (locked_page_allocations_kb / 1024) AS locked_pages_allocations_mb,
    (total_virtual_address_space_kb / 1024) AS total_virtual_address_space_mb,
    (virtual_address_space_committed_kb / 1024) AS committed_virtual_address_space_mb
FROM sys.dm_os_process_memory;
